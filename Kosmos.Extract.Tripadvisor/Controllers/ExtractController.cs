using HtmlAgilityPack;
using Kosmos.Extract.Tripadvisor.DbContext;
using Kosmos.Singleton;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kosmos.Extract.Tripadvisor.Controllers
{
    public class ExtractController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post(DownloadedResult downloadResult)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(downloadResult.Result);
            var nodes = doc.DocumentNode;

            if (null == nodes.SelectSingleNode("//*[@id='HEADING']"))
                return Ok();

            try
            {
                var images = ExtractImages(downloadResult.Result);
                var model = new
                {
                    //该数据所在url
                    url = downloadResult.Url,
                    //名字
                    name = nodes.SelectSingleNode("//*[@id='HEADING']")?.InnerText,
                    //评分
                    ratingValue = nodes.SelectSingleNode("//*[@id='HEADING_GROUP']/div/div[2]/div[1]/div/span/img")?.Attributes["content"]?.Value?.Replace("\n", ""),
                    //具体评分
                    ratingValueDetial = nodes.SelectSingleNode("//*[@id='ratingFilter']/ul")?.InnerText?.Replace("\n", ""),
                    //评论数
                    reviewCount = nodes.SelectSingleNode("//*[@id='HEADING_GROUP']/div/div[2]/div[1]/div/a")?.Attributes["content"]?.Value,
                    //参考价
                    referencePrice = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[1]/div[2]/div[2]/span")?.InnerText?.Replace("\n", ""),
                    //菜系
                    styleOfCooking = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[1]/div[3]/div[2]")?.InnerText?.Replace("\n", ""),
                    //营业时间
                    businessHours = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[1]/div[7]/div[2]")?.InnerText?.Replace("\n", ""),
                    //地址
                    address = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/ul/li[1]/div/span/span")?.InnerText?.Replace("\n", ""),
                    //地址详细信息
                    detial = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/ul/li[2]/div")?.InnerText?.Replace("\n", ""),
                    //联系方式
                    phone = nodes.SelectSingleNode("//*[@id='BODYCON']/div[2]/div/div[2]/div[2]/div[1]/div[1]/div[2]/div[2]/div[2]/ul/li[4]/div/span")?.InnerText?.Replace("\n", ""),
                    //所有图片
                    images = JsonConvert.SerializeObject(images?.Where(x => x.data.IndexOf(".jpg") > 0).Select(x => new { x.data, x.id }).ToList()),
                    //所有评论
                    comments = ExtractComment(doc, images),
                };

                ExtractResultCache.Result.Add(JsonConvert.SerializeObject(model));
            }
            catch (Exception e)
            {
                SingleHttpClient.PostException(e);
            }
            return Ok();
        }


        private List<Images> ExtractImages(string html)
        {
            try
            {
                var start = html.IndexOf("lazyImgs");
                var end = html.IndexOf("lazyHtml");
                var subHtml = html.Substring(start, end - start);
                subHtml = subHtml.Substring(subHtml.IndexOf("=") + 1, subHtml.LastIndexOf(";") - subHtml.IndexOf("=") - 1);
                subHtml = subHtml.Replace("\n", "").Replace(" ", "");

                var images = JsonConvert.DeserializeObject<List<Images>>(subHtml);

                return images;
            }
            catch (Exception e)
            {
                SingleHttpClient.PostException(e);

                return null;
            }
        }

        private string ExtractComment(HtmlDocument doc, List<Images> images)
        {
            try
            {
                var docCom = new HtmlDocument();
                var comments = doc?
                    .DocumentNode?
                    .SelectNodes("//*[contains(@class,'reviewSelector')]")?
                    .Select(html =>
                    {
                        docCom.LoadHtml(html.InnerHtml);
                        var nodes = docCom?.DocumentNode;
                        var partial_entry = nodes?.SelectNodes("//*[contains(@class,'partial_entry')]")?.First()?.InnerText;
                        if (null == partial_entry)
                            return null;
                        var id = nodes?.SelectNodes("//*[contains(@class,'avatar')]")?.FindFirst("img")?.Attributes["id"]?.Value;

                        var image = images?.FirstOrDefault(x => x.id == id);
                        if (null != image)
                        {
                            images.Remove(image);
                        }
                        return new
                        {
                        //头像
                        avatar = image?.data,
                        //用户名
                        username = nodes?.SelectNodes("//*[contains(@class,'username')]")?.FindFirst("span")?.InnerText?.Replace("\n", ""),
                        //用户国籍
                        location = nodes?.SelectNodes("//*[contains(@class,'location')]")?.First()?.InnerText?.Replace("\n", ""),
                        //用户等级
                        level = nodes?.SelectNodes("//*[contains(@class,'levelBadge')]")?.First()?.Attributes["class"].Value?.Replace("levelBadge", "")?.Replace("badge", "")?.Replace("lvl_", ""),
                        //用户总共评价次数
                        commentCount = nodes?.SelectNodes("//*[contains(@class,'badgeText')]")?.First()?.InnerText?.Replace("条点评", "")?.Replace("\n", ""),
                        //用户总共贫家餐厅次数
                        commentRestaurantCount = nodes?.SelectNodes("//*[contains(@class,'contributionReviewBadge')]")?.First()?.InnerText.Replace("条餐厅点评", "").Replace("\n", ""),
                        //被其他用户点赞次数
                        helpfulVotes = nodes?.SelectNodes("//*[contains(@class,'helpfulVotesBadge')]")?.First()?.InnerText?.Replace("人推荐", "").Replace("\n", ""),
                        //评论标题
                        title = nodes?.SelectNodes("//*[contains(@class,'noQuotes')]")?.First()?.InnerText?.Replace("\n", ""),
                        //评分
                        rate = nodes?.SelectNodes("//*[contains(@class,'sprite-rating_s_fill')]")?.First()?.Attributes["alt"]?.Value,
                        //评论时间
                        ratingDate = nodes?.SelectNodes("//*[contains(@class,'ratingDate')]")?.First()?.InnerText,
                        //评论内容
                        comment = nodes?.SelectNodes("//*[contains(@class,'partial_entry')]")?.First()?.InnerText
                        };
                    })
                    .Where(x => null != x)
                    .ToList();

                return JsonConvert.SerializeObject(comments);
            }
            catch (Exception e)
            {
                SingleHttpClient.PostException(e);
                return null;
            }
        }

        [HttpGet]
        [Route("api/Extract/CacheToDb")]
        public async Task<IHttpActionResult> CacheToDb()
        {
            using (var dbContext = new AppDbContext())
            {
                ExtractResultCache.CacheToDb(dbContext);
                return Ok("ok");
            }
        }
    }
}
