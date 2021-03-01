using System;
using System.Collections.Generic;
using libsecondlife;

namespace libsecondlife.TestClient
{
    public class AttachmentsCommand : Command
    {
        public AttachmentsCommand(TestClient testClient)
        {
            Client = testClient;
            Name = "attachments";
            Description = "Prints a list of the currently known agent attachments";
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            List<Primitive> attachments = Client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim) { return prim.ParentID == Client.Self.LocalID; }
            );

            for (int i = 0; i < attachments.Count; i++)
            {
                Primitive prim = attachments[i];
                AttachmentPoint point = Helpers.StateToAttachmentPoint(prim.Data.State);

                // TODO: Fetch properties for the objects with missing property sets so we can show names
                Logger.Log(String.Format("[Attachment @ {0}] LocalID: {1} UUID: {2} Offset: {3}",
                    point, prim.LocalID, prim.ID, prim.Position), Helpers.LogLevel.Info, Client);
            }

            return "Found " + attachments.Count + " attachments";
        }
    }
}
