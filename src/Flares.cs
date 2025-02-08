using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Flares
{
    public class Flares : EntityThrownStone
    {
        public Flares()
        {
            HorizontalImpactBreakChance = 0;
            LightHsv = new byte[] { 0, 6, 30 };

            /*CanCollect(Entity byEntity);*/
        }

    }
}
