using UnityEngine;
using Verse;

namespace RimWood
{
    /// <summary>
    /// Mod settings for RimWood.
    /// Allows players to configure fuel usage behavior.
    /// </summary>
    public class RimWoodSettings : ModSettings
    {
        /// <summary>
        /// If true, vanilla wood logs can be used as fuel in campfires, torches, and workbenches.
        /// If false, only processed fuels (firewood, charcoal) are accepted.
        /// Default: true (vanilla behavior).
        /// </summary>
        public bool allowLogsInBuildings = true;

        /// <summary>
        /// Persists settings across save/load.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref allowLogsInBuildings, "allowLogsInBuildings", true);
        }
    }

    /// <summary>
    /// Mod entry point for RimWood settings UI.
    /// </summary>
    public class RimWoodModController : Mod
    {
        public static RimWoodSettings settings;

        public RimWoodModController(ModContentPack content) : base(content)
        {
            settings = GetSettings<RimWoodSettings>();
        }

        /// <summary>
        /// Draws the mod settings window.
        /// </summary>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.CheckboxLabeled(
                "Allow logs in fuel buildings",
                ref settings.allowLogsInBuildings,
                "If enabled, vanilla wood logs can be used as fuel in campfires, torches, and workbenches (same as vanilla behavior). If disabled, only processed fuels (firewood and charcoal) are accepted, forcing a more realistic fuel economy."
            );

            listingStandard.Gap(12f);
            listingStandard.Label("Note: Changing this setting may affect existing fuel hauling jobs. Buildings with already-loaded fuel will keep it, but new fuel deliveries will respect this setting.");

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Name displayed in mod settings menu.
        /// </summary>
        public override string SettingsCategory()
        {
            return "RimWood";
        }
    }
}
