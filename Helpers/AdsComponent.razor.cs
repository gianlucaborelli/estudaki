using Microsoft.AspNetCore.Components;

namespace ProvaOnline.Helpers
{
    public partial class AdsComponent : ComponentBase
    {
        [Parameter]
        public string Slot { get; set; }
        [Parameter]
        public string Style { get; set; }
        [Parameter]
        public string Adsformat { get; set; } = "auto";

        protected RenderFragment Ads { get; set; }

        protected override void OnParametersSet()
        {
            Ads = new RenderFragment(b =>
            {
                b.OpenElement(0, "ins");
                b.AddMultipleAttributes(1, new List<KeyValuePair<string, object>>()
                {
                new KeyValuePair<string, object>("class", "adsbygoogle"),
                new KeyValuePair<string, object>("style", $"{Style}"),
                new KeyValuePair<string, object>("data-ad-client", "ca-pub-9306950234366060"),
                new KeyValuePair<string, object>("data-ad-slot", Slot),
                new KeyValuePair<string, object>("data-ad-format", Adsformat),
                new KeyValuePair<string, object>("data-full-width-responsive", true),
                });
                b.CloseElement();

                b.OpenElement(0, "script");
                b.AddContent(3, "(adsbygoogle = window.adsbygoogle || []).push({});");
                b.CloseElement();
            });
        }
    }
}
