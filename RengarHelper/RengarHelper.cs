﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using System.Drawing;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System;
using System.Linq;
using SharpDX;
using Color1 = System.Drawing.Color;

namespace RengarHelper
{
    class Program
    {
        public static AIHeroClient _Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }
        public static bool AutoSnare
        {
            get { return HelpMenu["autoe5"].Cast<KeyBind>().CurrentValue; }
        }
        public static Slider SkinSelect;
        public static Menu Menu, DrawMenu, HelpMenu, HarassMenu, SkinMenu;
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static AIHeroClient Rengar
        {
            get { return ObjectManager.Player; }
        }
        public static int SliderValue(Menu obj, string value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }
        public static Text textDraw = new Text("", new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold));
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }
        private static void Game_OnStart(EventArgs args)
        {
            if (_Player.ChampionName == "Rengar")
            {
                Chat.Print("RengarHelper Loaded!", Color1.AliceBlue);
                Chat.Print("Made by Capitao Addon", Color1.LightBlue);
            }

            if (_Player.ChampionName != "Rengar")
            {
                Chat.Print("Y U NOT USING RENGO", Color1.Red);
                return;
            }

            Q = new Spell.Active(SpellSlot.Q, 125);
            W = new Spell.Active(SpellSlot.W, 500);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1500, 70);

            Menu = MainMenu.AddMenu("RengarHelper", "rengoh");
            Menu.AddLabel("Made by Capitão Addon");

            DrawMenu = Menu.AddSubMenu("Drawings", "draws");
            DrawMenu.Add("ddd", new CheckBox("Disable draws"));
            DrawMenu.Add("fpsd", new CheckBox("High quality draws"));
            DrawMenu.Add("dw", new CheckBox("Draw W", false));
            DrawMenu.Add("de", new CheckBox("Draw E"));

            HelpMenu = Menu.AddSubMenu("Helper", "help");
            HelpMenu.Add("autoq", new CheckBox("Auto Q in mid air"));
            HelpMenu.Add("autow", new CheckBox("Auto W in mid air"));
            HelpMenu.Add("autoe", new CheckBox("Auto E in mid air"));
            HelpMenu.Add("autoe5", new KeyBind("Auto snare in mid air", false, KeyBind.BindTypes.PressToggle, 'J'));

            HarassMenu = Menu.AddSubMenu("Harass", "har");
            HarassMenu.Add("hq", new CheckBox("Use Q in Harass"));
            HarassMenu.Add("hw", new CheckBox("Use W in Harass"));
            HarassMenu.Add("he", new CheckBox("Use E in Harass"));
            HarassMenu.Add("hpe", new CheckBox("Use empowered E"));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "skins");
            SkinSelect = SkinMenu.Add("skint", new Slider("Skin", 2, 0, 3));

            _Player.SetSkin(_Player.ChampionName, SliderValue(SkinMenu, "skint"));
            SkinSelect.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs skiniddd)
            {
                _Player.SetSkinId(skiniddd.NewValue);
            };


        }
        private static void Game_OnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (Player.Instance.IsDashing() && HelpMenu["autoe"].Cast<CheckBox>().CurrentValue && E.IsReady() && _Player.Mana <= 5)
            {
                E.Cast(target);
            }
            if (Player.Instance.IsDashing() && HelpMenu["autoe5"].Cast<KeyBind>().CurrentValue && _Player.Mana == 5 && E.IsReady())
            {
                E.Cast(target);
            }
            if (Player.Instance.IsDashing() && HelpMenu["autoq"].Cast<CheckBox>().CurrentValue && _Player.Mana <= 5)
            {
                Q.Cast();
            }
            if (Player.Instance.IsDashing() && HelpMenu["autow"].Cast<CheckBox>().CurrentValue && _Player.Mana <= 5 && W.Range < _Player.Distance(target)) 
            {
                W.Cast();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (!target.IsValid) return;

                if (HarassMenu["hq"].Cast<CheckBox>().CurrentValue && !Orbwalker.IsAutoAttacking && Q.IsReady() && _Player.Distance(target) <= Q.Range && _Player.Mana <= 5)
                {
                    Q.Cast();
                }
                if (HarassMenu["hw"].Cast<CheckBox>().CurrentValue && W.IsReady() && _Player.Distance(target) <= W.Range && _Player.Mana <= 5)
                {  
                    W.Cast();
                }
                if (HarassMenu["he"].Cast<CheckBox>().CurrentValue && E.IsReady() && _Player.Distance(target) <= E.Range && _Player.Mana <= 5)
                {
                    E.Cast(target);
                }
                if (HarassMenu["hpe"].Cast<CheckBox>().CurrentValue && _Player.Distance(target) <= E.Range && _Player.Mana == 5)
                {
                    E.Cast(target);
                }
            }
            //255 730

        }


        private static void Game_OnDraw(EventArgs args)
        {

            if (DrawMenu["dw"].Cast<CheckBox>().CurrentValue && !DrawMenu["fpsd"].Cast<CheckBox>().CurrentValue && !DrawMenu["ddd"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(W.IsReady() ? SharpDX.Color.WhiteSmoke : SharpDX.Color.Tomato, 500, Player.Instance.Position);
            }
            if (DrawMenu["de"].Cast<CheckBox>().CurrentValue && !DrawMenu["fpsd"].Cast<CheckBox>().CurrentValue && !DrawMenu["ddd"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(E.IsReady() ? SharpDX.Color.WhiteSmoke : SharpDX.Color.Tomato, 1000, Player.Instance.Position);
            }
            if (DrawMenu["de"].Cast<CheckBox>().CurrentValue && DrawMenu["fpsd"].Cast<CheckBox>().CurrentValue && !DrawMenu["ddd"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, E.Range, E.IsReady() ? Color1.Beige : Color1.Tomato);
            }
            if (DrawMenu["dw"].Cast<CheckBox>().CurrentValue && DrawMenu["fpsd"].Cast<CheckBox>().CurrentValue && !DrawMenu["ddd"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, W.Range, W.IsReady() ? Color1.Beige : Color1.Tomato);
            }
            var pos = Drawing.WorldToScreen(Player.Instance.Position);

            if (AutoSnare)
            {
                textDraw.Draw("Auto snare ON", SharpDX.Color.White, (int)pos.X - 45,
                   (int)pos.Y + 40);
            }
            else textDraw.Draw("Auto snare OFF", SharpDX.Color.OrangeRed, (int)pos.X - 45, (int)pos.Y + 40);

        }
    }
}
