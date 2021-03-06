﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace KSPMod
{
    class SMUI : MonoBehaviour
    {
        private GUIStyle g_window;
        private GUIStyle g_toggle;
        private GUIStyle g_box;
        private GUIStyle g_button;
        private GUIStyle g_tagEntry;
        private GUIStyle g_craftEntry;
        private GUIStyle g_scrollbar;
        private GUIStyle g_label;
        private GUIStyle g_textfield;

        public void Start()
        {
            g_window = HighLogic.Skin.window;
            g_toggle = HighLogic.Skin.toggle;
            g_box = HighLogic.Skin.box;
            g_button = HighLogic.Skin.button;
            g_scrollbar = HighLogic.Skin.verticalScrollbar;
            g_label = HighLogic.Skin.label;
            g_textfield = HighLogic.Skin.textField;

            g_tagEntry = g_button;

            g_craftEntry = new GUIStyle(HighLogic.Skin.customStyles[0]);
            g_craftEntry.fontSize = 12;
            g_craftEntry.alignment = TextAnchor.MiddleLeft;

            UILogic.queryText = "";
        }

        public void OnGUI()
        {
            if (UILogic.enableOriginal == false)
            {
                GUI.Window(0, new Rect(Screen.width / 2.0f - 320, Screen.height / 2.0f - 250, 140, 500), TagWindow, "Select Tag", g_window);
                GUI.Window(1, new Rect(Screen.width / 2.0f - 170, Screen.height / 2.0f - 250, 400, 500), CraftWindow, "Select Craft", g_window);
                if(UILogic.selectedShip != null)
                    GUI.Window(2, new Rect(Screen.width / 2.0f + 240, Screen.height / 2.0f - 250, 200, 240), PreviewWindow, "Preview", g_window);
            }
            else
            {
                if (UILogic.originalBrowser == null)
                {
                    UILogic.ReloadShipData();   // Case where ship deletion cannot be detected, so reload
                    destroy();
                }
                else if (GUI.Button(new Rect(Screen.width / 2.0f + 150, Screen.height / 2.0f - 240, 20, 20), "O", g_button))
                    UILogic.enableOriginal = false;
            }
        }

        private Vector2 tagScrollPosition = Vector2.zero;
        public void TagWindow(int id)
        { 
            const int ITEMHEIGHT = 30;
            int i = 0;
            var list = UILogic.Tag_getList();
            tagScrollPosition = GUI.BeginScrollView(new Rect(5, 40, 130, 450), tagScrollPosition, new Rect(5, 0, 110, Mathf.Max(ITEMHEIGHT * list.Count, 450)), false, true);
            foreach (UI_TagWindow item in list)
            {
                item.update(GUI.Toggle(new Rect(5, ITEMHEIGHT* i, 110, (ITEMHEIGHT-5)), item.selected, item.tag, g_tagEntry));
                i++;
            }
            GUI.Box(new Rect(0, 0, 0, 0), "");
            GUI.EndScrollView();
        }

        private bool deleteConfirm = false;
        private Vector2 craftScrollPosition = Vector2.zero;
        public void CraftWindow(int id)
        {
            const int ITEMHEIGHT = 60;
            int i = 0;
            var list = UILogic.Craft_getList();
            craftScrollPosition = GUI.BeginScrollView(new Rect(5, 40, 390, 380), craftScrollPosition, new Rect(5, 0, 370, Mathf.Max(ITEMHEIGHT * list.Count, 400)), false, true);
            foreach (UI_CraftWindow item in list)
            {
                item.update(GUI.Toggle(new Rect(15, ITEMHEIGHT * i, 360, (ITEMHEIGHT - 5)), item.selected, item.craft.description, g_craftEntry));
                if(item.craft.thumbnail.width != 0)
                    GUI.Label(new Rect(370 - (ITEMHEIGHT - 10), ITEMHEIGHT * i + 5, ITEMHEIGHT - 10, ITEMHEIGHT - 10), item.craft.thumbnail);
                i++;
            }
            GUI.Box(new Rect(0, 0, 0, 0), "");
            GUI.EndScrollView();

            if (GUI.Button(new Rect(320, 460, 70, 30), "<color=#CCFF00>Load</color>", g_button))
            {
                if(UILogic.LoadSelectedShip(CraftBrowser.LoadType.Normal))
                    destroy();
            }


            if (GUI.Button(new Rect(240, 460, 70, 30), "<color=#F79303>Merge</color>", g_button))
            {
                if(UILogic.LoadSelectedShip(CraftBrowser.LoadType.Merge))
                    destroy();
            }

            if (GUI.Button(new Rect(10, 460, 70, 30), "<color=#FF0000>Delete</color>", g_button))
                if (UILogic.selectedShip == null)
                    deleteConfirm = false;
                else
                    deleteConfirm = !deleteConfirm;

            if (deleteConfirm)
            {
                GUI.Label(new Rect(90, 460, 120, 30), "Are you sure?");
                if (GUI.Button(new Rect(180, 460, 30, 30), "<color=#FF0000>Y</color>", g_button))
                {
                    UILogic.DeleteSelectedShip();
                    deleteConfirm = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(90, 460, 70, 30), "Cancel", g_button))
                    destroy();
            }

            if (GUI.Button(new Rect(370, 10, 20, 20), "O", g_button))
                UILogic.enableOriginal = true;

            GUI.SetNextControlName("QueryText");
            UILogic.queryText = GUI.TextField(new Rect(10, 430, 380, 20), UILogic.queryText, g_textfield);
            if (UILogic.queryText == null || UILogic.queryText == "")
                GUI.Label(new Rect(10, 430, 90, 20), "<i>  search craft...</i>");
        }

        public void PreviewWindow(int id)
        {
            if(UILogic.selectedShip != null && UILogic.selectedShip.thumbnail.width != 0)
                GUI.Box(new Rect(10, 50, 180, 180), UILogic.selectedShip.thumbnail, g_box);
        }

        private void destroy()
        {
            if(UILogic.originalBrowser != null)
                UILogic.originalBrowser.OnBrowseCancelled();
            UILogic.onDestroy();
            Destroy(this);
        }
       
    }
}
