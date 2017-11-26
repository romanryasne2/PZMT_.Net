using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ColorInteraction.Common;
using ColorInteraction.Web.Models;

namespace ColorInteraction.Web.Controllers
{
    public class ColorController : ApiController
    {
        [HttpGet]
        public async Task<Color> CurrentColor(DateTime lastUpdate)
        {
            List<ColorMessage> colorMessages = await new Database.Database()
                .GetColorMessages(lastUpdate);

            if (colorMessages.Count == 0)
            {
                return new Color();
            }

            byte r = (byte) colorMessages.Average(item => item.Color.R);
            byte g = (byte) colorMessages.Average(item => item.Color.G);
            byte b = (byte) colorMessages.Average(item => item.Color.B);

            return new Color
            {
                R = r,
                B = b,
                G = g
            };
        }

        [HttpGet]
        public async Task<IEnumerable<ColorMessageModel>> Actors(DateTime lastUpdate)
        {
            return (await new Database.Database()
                .GetColorMessages(lastUpdate))
                .Select(item => new ColorMessageModel
                {
                    MachineName = item.MachineName,
                    Color = new Color
                    {
                        R = item.Color.R,
                        G = item.Color.G,
                        B = item.Color.B
                    }
                });
        }
    }
}