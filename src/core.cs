using Vintagestory.API.Common;

[assembly: ModInfo("Flares")]

namespace Flares
{
    class Core : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterEntity("EntityFlare", typeof(Flares));
            api.RegisterItemClass("ItemFlare", typeof(ItemFlare));

            api.World.Logger.Event("started 'Flares' mod");
        }
    }
}