using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using Nuztalgia.StardewMods.Common.GenericModConfigMenu;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class ConfigMenuHelper {

  private readonly Integration ConfigMenu;
  private readonly IGameContentHelper GameContentHelper;

  private readonly Func<string, string> FormatValue =
      value => Globals.GetI18nString($"Value_{value}") ?? value;

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

    this.ConfigMenu.OnFieldChanged((string fieldId, object newValue) => {
      Log.Verbose($"Field '{fieldId}' was changed to: '{newValue}'.");
      ImagePreviewOptions.SetFieldValue(fieldId, newValue);
    });
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
        this.ConfigMenu.AddParagraph(() =>
            syncedSection.GetSyncedItems() is IEnumerable<string> syncedItems
                ? syncedItems.Any()
                    ? "> " + string.Join(" \n > ", syncedItems) // TODO: Display these prettily.
                    : I18n.Compat_Synced_None()
                : I18n.Compat_Synced_None());
      } else {
        this.AddSectionOptions(section);
      }

      this.ConfigMenu.AddSpacing();
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
      IEnumerable<BaseMenuSection.OptionItem> optionItems = character.GetOptions();

      ImagePreviewOptions.InitializeCharacter(
          character.Name,
          this.GameContentHelper.Load<Texture2D>,
          contentPackPage.GetImageLoader(), 
          character.GetModImagePaths,
          character.GetGameImagePaths,
          character.GetPortraitRectsDelegate(),
          character.GetSpriteRectsDelegate());

      this.AddSectionOptions(character, optionItems)
          .AddComplexOption(
              optionName: " =  " + I18n.Option_Preview(),
              tooltip: character.GetPreviewTooltip(),
              getHeight: () => ImagePreviewOptions.GetHeight(character.Name),
              drawAction: (spriteBatch, position) =>
                  ImagePreviewOptions.Draw(character.Name, spriteBatch, position),
              resetAction: () => ImagePreviewOptions.ResetState(character.Name),
              saveAction: () => ImagePreviewOptions.SaveState(character.Name));

      optionItems.ForEach(item => ImagePreviewOptions.SetFieldValue(item.UniqueId, item.Value));
      ImagePreviewOptions.SaveState(character.Name);
    }
  }

  private Integration AddSectionOptions(BaseMenuSection section) {
    return this.AddSectionOptions(section, section.GetOptions());
  }

  private Integration AddSectionOptions(
      BaseMenuSection section, IEnumerable<BaseMenuSection.OptionItem> optionItems) {
    foreach (BaseMenuSection.OptionItem item in optionItems) {
      string displayName = " >  " + item.Name;
      switch (item.Value) {
        case Enum: {
          this.ConfigMenu.AddEnumOption(
              section, item.Property, displayName, this.FormatValue, item.Tooltip, item.UniqueId);
          break;
        }
        case bool: {
          this.ConfigMenu.AddBoolOption(
              section, item.Property, displayName, item.Tooltip, item.UniqueId);
          break;
        }
        case int: {
          (int min, int max) = section.GetValueRange(item.Property);
          this.ConfigMenu.AddIntOption(
              section, item.Property, displayName, min, max, item.Tooltip, item.UniqueId);
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
