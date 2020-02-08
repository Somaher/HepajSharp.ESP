using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HepajSharp.HepajSharp;
using HepajSharp.HepajSharp.Entities;
using HepajSharp.HepajSharp.Utils;
using HepajSharp.HepajSharpKernel.Interfaces;
using Bones = HepajSharp.HepajSharp.Enumerations.Definitions.ECSPlayerBones;

namespace HepajSharp.ESP
{
    public class Program
    {
        private static GUIManager.MenuItem menu = new GUIManager.MenuItem();
        public static void Main()
        {
            menu.Name = "Vision Assistance";
            var active = new GUIManager.ToggleMenu("Activate", true);
            active.SetParent(menu);
            GUIManager.AddToRoot(menu);
            PaintTraverse.AfterPaintTraverse += PaintTraverse_AfterPaintTraverse;
        }

        private static void PaintTraverse_AfterPaintTraverse(IntPtr pPanel, uint vguiPanel)
        {
            if (!(menu.Children[0] as GUIManager.ToggleMenu).IsToggled())
                return;

            for (int i = 1; i < CEngineClient.Instance.GetMaxClients(); i++)
            {
                if (i == CEngineClient.Instance.GetLocalPlayer())
                    continue;
                        
                var pTarget = new C_CSPlayer(i);    //TODO: cache this to Array
                        
                if (!pTarget.IsPlayer())
                    continue;

                if (!pTarget.IsAlive() || pTarget.IsDormant() || !pTarget.IsEnemy())
                    continue;

                var vOrigin = pTarget.GetOrigin();
                var vHead = Utils.Utils.GetEntityBone(pTarget, Bones.HEAD_0);
                vHead.Z += 14;
                        
                Vector3 vScreenOrigin = new Vector3(), vScreenHead = new Vector3();
                if (!Utils.Utils.WorldToScreen(vHead, ref vScreenHead) || !Utils.Utils.WorldToScreen(vOrigin, ref vScreenOrigin))
                    continue;

                var height = Math.Abs(vScreenHead.Y - vScreenOrigin.Y);
                var width = height * 0.65f;
                Drawing.DrawOutlineRect((int)vScreenHead.X - (int)width / 2, (int)vScreenHead.Y, (int)width, (int)height, new Color(255,255,255,255));
                Drawing.DrawText((int)vScreenOrigin.X,(int)vScreenOrigin.Y, pTarget.GetName(i), new Color(255,255,255), true);
            }
        }
    }
}
