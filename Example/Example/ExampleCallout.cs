using LSPD_First_Response.Mod.Callouts;
using Rage;

namespace ExampleCallouts
{
    [CalloutInfo("Example Callout", CalloutProbability.Never)]
    public class ExampleCallout : Callout
    {
        private bool isUpdateFiberStarted = false;

        public override bool OnBeforeCalloutDisplayed()
        {
            /*
             *  Do not make any calls to Callout Inteface in OnBeforeCalloutDisplayed.
             */
            this.CalloutMessage = "This message will be displayed first in the MDT";
            this.CalloutAdvisory = "If this is set and different than CalloutMessage, it will be displayed second in the MDT.";
            this.CalloutPosition = World.GetRandomPositionOnStreet();
            return base.OnBeforeCalloutDisplayed();
        }

        public override void OnCalloutDisplayed()
        {
            /*
             *  Here you can send CalloutInterface some additional details about the callout such as the intended agency
             *  and the priority (e.g. CODE 2, CODE 3).  If you do not supply an agency or send an empty string, Callout
             *  Interface will assume the agency matches the player if/when the callout is accepted.
             */
            if (PluginChecker.IsCalloutInterfaceRunning)
            {
                CalloutInterfaceFunctions.SendCalloutDetails(this, "CODE 2", "LSPD");
            }

            base.OnCalloutDisplayed();
        }

        public override void OnCalloutNotAccepted()
        {
            /*
             *  A lot of callouts play scanner audio here indicating another unit has accepted.  However, one of the goals of
             *  Callout Interface is to allow players to explicitly reject callouts while also having some go into a backlog
             *  that they can work later if they so choose. As such, I recommend doing a check here to see if Callout Interface
             *  is running and only playing the OTHER_UNIT_TAKING_CALL audio if it is not.
             */
            if (!PluginChecker.IsCalloutInterfaceRunning)
            {
                LSPD_First_Response.Mod.API.Functions.PlayScannerAudio("OTHER_UNIT_TAKING_CALL");
            }

            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (!this.isUpdateFiberStarted)
            {
                this.isUpdateFiberStarted = true;
                GameFiber.StartNew(UpdateFiber, "YOU-SHOULD-ALWAYS-NAME-YOUR-FIBERS");
            }

            base.Process();
        }

        private void UpdateFiber()
        {
            /*
             *  This will send a message to the MDT every 20 seconds.
             */
            int counter = 0;
            while (this.AcceptanceState == CalloutAcceptanceState.Running)
            {
                if (++counter % 20 == 0 && PluginChecker.IsCalloutInterfaceRunning)
                {
                    CalloutInterfaceFunctions.SendMessage(this, "Callout update");
                }
                GameFiber.Sleep(1000);
            }
        }
    }
}
