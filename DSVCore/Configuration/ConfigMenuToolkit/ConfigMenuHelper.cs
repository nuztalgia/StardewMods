using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class ConfigMenuHelper {

  private static readonly string PreviewLabel = " =  " + I18n.Option_Preview();
  private static readonly Func<string, string> FormatValue =
      value => Globals.GetI18nString($"Value_{value}") ?? value;

  private readonly Integration ConfigMenu;
  private readonly IGameContentHelper GameContentHelper;

  internal ConfigMenuHelper(Integration genModConfigMenu, IGameContentHelper gameContentHelper) {
    this.ConfigMenu = genModConfigMenu;
    this.GameContentHelper = gameContentHelper;
  }

  internal void SetUpMenuPages(
      CoreAndCompatPage coreAndCompatPage, 
      IEnumerable<BaseContentPackPage> installedContentPackPages,
      IEnumerable<BaseContentPackPage> otherContentPackPages) {

    this.SetUpMainPage(coreAndCompatPage, installedContentPackPages, otherContentPackPages);
    this.SetUpCoreAndCompatPage(coreAndCompatPage);
    installedContentPackPages.ForEach(page => this.SetUpContentPackPage(page));

    this.ConfigMenu.OnFieldChanged(OnFieldChanged);
  }

  private static void OnFieldChanged(string fieldId, object newValue) {
    Log.Trace($"Field '{fieldId}' was changed to: '{newValue}'.");
    CharacterConfigState.Update(fieldId, newValue);
  }

  private void SetUpMainPage(
      CoreAndCompatPage coreAndCompatPage,
      IEnumerable<BaseContentPackPage> installedContentPackPages,
      IEnumerable<BaseContentPackPage> otherContentPackPages) {

    this.ConfigMenu
        .AddSectionTitle(I18n.Main_Intro_Title)
        .AddParagraph(I18n.Main_Intro_Description)
        .AddPageLink(coreAndCompatPage.Name, $" > {coreAndCompatPage.GetDisplayName()}")
        .AddSpacing()
        .AddSectionTitle(I18n.Main_InstalledPacks_Title)
        .AddParagraph(installedContentPackPages.Any()
                      ? I18n.Main_InstalledPacks_Description
                      : I18n.Main_InstalledPacks_None);

    foreach (BaseContentPackPage contentPackPage in installedContentPackPages) {
      Log.Trace($"'{contentPackPage.GetDisplayName()}' pack is installed. Adding config menu.");
      this.ConfigMenu.AddPageLink(contentPackPage.Name, $" > {contentPackPage.GetDisplayName()}");
    }

    this.ConfigMenu
        .AddSpacing()
        .AddSectionTitle(I18n.Main_OtherPacks_Title)
        .AddParagraph(otherContentPackPages.Any()
                      ? I18n.Main_OtherPacks_Description
                      : I18n.Main_OtherPacks_None);

    foreach (BaseContentPackPage contentPackPage in otherContentPackPages) {
      Log.Trace($"'{contentPackPage.GetDisplayName()}' pack is *NOT* installed.");
      this.ConfigMenu.AddSectionTitle($" * {contentPackPage.GetDisplayName()}");
    }
  }

  private void SetUpCoreAndCompatPage(CoreAndCompatPage coreAndCompatPage) {
    BaseMenuSection coreOptions = coreAndCompatPage.CoreOptions;
    IEnumerable<BaseCompatSection> compatSections = coreAndCompatPage.GetAvailableCompatSections();

    this.ConfigMenu
        .AddPage(coreAndCompatPage.Name, coreAndCompatPage.GetDisplayName())
        .AddSectionTitle(coreOptions.GetDisplayName())
        .AddParagraph(I18n.Core_Section_Description);

    this.AddSectionOptions(coreOptions)
        .AddSpacing();

    foreach (BaseCompatSection section in compatSections) {
      this.ConfigMenu
          .AddSectionTitle(section.GetDisplayName())
          .AddParagraph(section.GetInfoText());

      if (section is BaseSyncedCompatSection syncedSection) {
        this.ConfigMenu.AddCharacterThumbnails(
            syncedSection, this.GameContentHelper.Load<Texture2D>);
      } else {
        this.AddSectionOptions(section).AddSpacing();
      }
    }

    // Show the placeholder if there are no compat mods or if the only one is Flower Queen's Crown.
    if (!compatSections.Any()
        || (compatSections.Count() == 1 && compatSections.First().Name == "FlowerQueensCrown")) {
      this.ConfigMenu
          .AddSectionTitle(I18n.Compat_Placeholder_Title)
          .AddParagraph(I18n.Compat_Placeholder_Description);
    }
  }

  private void SetUpContentPackPage(BaseContentPackPage contentPackPage) {
    this.ConfigMenu.AddPage(contentPackPage.Name, contentPackPage.GetDisplayName());

    foreach (BaseCharacterSection character in contentPackPage.GetAllSections()) {
      this.ConfigMenu.AddSectionTitle(character.GetDisplayName().CapitalizeFirstChar());

      this.AddSectionOptions(character)
          .AddCharacterPreview(
              character,
              PreviewLabel,
              this.GameContentHelper.Load<Texture2D>,
              contentPackPage.GetImageLoader())
          .AddSpacing();
    }
  }

  private Integration AddSectionOptions(BaseMenuSection section) {
    foreach (BaseMenuSection.OptionItem item in section.GetOptions()) {
      string optionName = " >  " + item.Name;
      switch (item.Value) {
        case Enum: {
          this.ConfigMenu.AddEnumOption(
              section, item.Property, optionName, FormatValue, item.Tooltip, item.UniqueId);
          break;
        }
        case bool: {
          this.ConfigMenu.AddBoolOption(
              section, item.Property, optionName, item.Tooltip, item.UniqueId);
          break;
        }
        case int value: {
          this.ConfigMenu.AddDynamicSlider(
              section, item.Property, optionName, value,
              OnFieldChanged, item.Tooltip, item.UniqueId);
          break;
        }
        default: {
          Log.Error($"Unexpected type '{item.Value?.GetType()}' for option '{item.UniqueId}'.");
          break;
        }
      }
    }
    return this.ConfigMenu;
  }
}
