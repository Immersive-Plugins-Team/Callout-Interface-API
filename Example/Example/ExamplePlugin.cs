using LSPD_First_Response.Mod.API;

[assembly: Rage.Attributes.Plugin("ExamplePlugin", Description = "An example plugin.", Author = "Immersive Plugins Team")]
namespace ExampleCallouts
{
    public class ExamplePlugin : Plugin
    {
        public override void Finally() { }

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += Functions_OnOnDutyStateChanged;
        }

        private void Functions_OnOnDutyStateChanged(bool onDuty)
        {
            if (onDuty)
            {
                Functions.RegisterCallout(typeof(ExampleCallout));
            }
        }
    }
}
