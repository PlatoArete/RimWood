using UnityEngine;
using Verse;

namespace RimWood
{
    /// <summary>
    /// A simple modal window that renders a ThingFilterUI for the per-building fuel filter.
    /// </summary>
    public class Dialog_FuelFilter : Window
    {
        private readonly ThingFilter filter;
        private readonly ThingFilter parentFilter;
        private ThingFilterUI.UIState uiState = new ThingFilterUI.UIState();

        public Dialog_FuelFilter(ThingFilter filter, ThingFilter parentFilter)
        {
            this.filter = filter;
            this.parentFilter = parentFilter;
            doCloseButton = true;
            forcePause = false;
            absorbInputAroundWindow = false;
            closeOnClickedOutside = true;
        }

        public override Vector2 InitialSize => new Vector2(300f, 480f);

        public override void DoWindowContents(Rect inRect)
        {
            Rect filterRect = new Rect(inRect);
            filterRect.yMax -= CloseButSize.y + StandardMargin;
            ThingFilterUI.DoThingFilterConfigWindow(filterRect, uiState, filter, parentFilter);
        }
    }
}
