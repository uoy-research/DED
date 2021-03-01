using System;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;

namespace libsecondlife.TestClient
{
    public class IndividualAnimationCommand : Command
    {
        LLUUID targetID;
        InventoryFolder rootfolder;

        public IndividualAnimationCommand(TestClient testClient)
        {
            Name = "individualanimation";
            Description = "CURRENTLY BROKEN!!! plays or stops an individual animation.\nUsage: individualanimation start/stop [animationname] or individualanimation list\ne.g.: individualanimation start latin salsa13";
            
        }

        public override string Execute(string[] args, LLUUID fromAgentID)
        {
            return "broken with current version of libsecondlife - needs to be fixed.";
            /*
            if (args.Length < 1)
                return Description;
            rootfolder = Client.Inventory.getFolder(@"Animations");
            rootfolder.RequestDownloadContents(true, true, true, 10000);
            if (args[0] == "list")
            {
                StringBuilder result = new StringBuilder();
                PrintFolder(rootfolder, result, 0);
                return result.ToString();
            }
            string animation_name = String.Empty;
            for (int ct = 1; ct < args.Length; ct++)
                animation_name += args[ct] + " ";
            animation_name = animation_name.TrimEnd();

            targetID = new LLUUID();
            getAnimationID(animation_name, rootfolder);

            if (args[0] == "start")
                Client.Self.AnimationStart(targetID);
            else if (args[0] == "stop")
                Client.Self.AnimationStop(targetID);
            else
                return Description;
            return "Done.";
             * */
        }

        void getAnimationID(String name, InventoryFolder folder)
        {
            /*
            foreach (InventoryBase b in folder.GetContents())
            {
                InventoryItem item = b as InventoryItem;
                if (item != null)
                {
                    if (item.Name == name)
                    {
                        targetID = item.AssetID;
                        break;
                    }
                    continue;
                }
                InventoryFolder subFolder = b as InventoryFolder;
                if (subFolder != null)
                    getAnimationID(name, subFolder);
            }
             * */
        }


        void PrintFolder(InventoryFolder folder, StringBuilder output, int indenting)
        {
            /*
            Indent(output, indenting);
            output.Append(folder.Name);
            output.Append("\n");
            foreach (InventoryBase b in folder.GetContents())
            {
                InventoryItem item = b as InventoryItem;
                if (item != null)
                {
                    Indent(output, indenting + 1);
                    output.Append(item.Name);
                    output.Append("\n");
                    continue;
                }
                InventoryFolder subFolder = b as InventoryFolder;
                if (subFolder != null)
                    PrintFolder(subFolder, output, indenting + 1);
            }
             * */
        }

        void Indent(StringBuilder output, int indenting)
        {
            for (int count = 0; count < indenting; count++)
            {
                output.Append("  ");
            }
        }
    }
}
