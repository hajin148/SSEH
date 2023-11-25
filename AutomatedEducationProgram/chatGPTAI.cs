using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace AutomatedEducation.Controllers
{
    public class chatGPTAI : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // new controller
        [HttpPost]
        [Route("getresult")]
        public IActionResult GetResult([FromBody]string promt)
        {
            string apikey = "#";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apikey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = promt;
            completion.Model = OpenAI_API.Models.Model.DefaultModel;
            completion.MaxTokens = 1000;
            var result = openai.Completions.CreateCompletionsAsync(completion);
            if (result != null)
            {
                foreach(var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
                return Ok(answer);
            }
            else
            {
                return BadRequest("Not Found");
            }
        }
    }
}
