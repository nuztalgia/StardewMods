using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Nuztalgia.StardewMods.Common;
using StardewModdingAPI;

namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class ConfigMenuHelper {

  private readonly GenericModConfigMenuIntegration ConfigMenu;
  private readonly IGameContentHelper GameContentHelper;

  private readonly Func<string, string> FormatValue =
      value => Globals.GetI18nString($"Value_{value}") ?? value;

  internal ConfigMenuHelper(
      GenericModConfigMenuIntegration gmcmIntegration, IGameContentHelper gameContentHelper) {
    this.ConfigMenu = gmcmIntegration;
    this.GameContentHelper = gameContentHelper;
  }

  internal void SetUpMenuPages(
      CoreAndCompatPage coreAndCompatPage, 
      IEnumerable<BaseContentPackPage> installedContentPackPages,
      IEnumerable<BaseContentPackPage> otherContentPackPages) {

    this.SetUpMainPage(coreAndCompatPage, installedContentPackPages, otherContentPackPages);
    this.SetUpCoreAndCompatPage(coreAndCompatPage);

    foreach (BaseContentPackPage contentPackPage in installedContentPackPages) {
      this.SetUpContentPackPage(contentPackPage);
    }

    this.ConfigMenu.OnFieldChanged((string fieldId, object newValue) => {
      Log.Verbose($"Field '{fieldId}' was changed to: '{newValue}'.");
      ImagePreviews.SetFieldValue(fieldId, newValue);
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
    IEnumerable<BaseMenuSection> compatSections = coreAndCompatPage.GetAvailableCompatSections();

    this.ConfigMenu
        .AddPage(coreAndCompatPage.Name, coreAndCompatPage.GetDisplayName())
        .AddSectionTitle(coreOptions.GetDisplayName())
        .AddParagraph(I18n.Core_Section_Description);

    this.AddSectionOptions(coreOptions)
        .AddSpacing();

    foreach (BaseMenuSection section in compatSections) {
      this.ConfigMenu.AddSectionTitle(section.GetDisplayName());
      this.AddSectionOptions(section)
          .AddSpacing();
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
      string displayName = character.GetDisplayName();
      this.ConfigMenu.AddSectionTitle( // Make sure the section title is capitalized.
          string.Concat(displayName[0].ToString().ToUpper(), displayName.AsSpan(1)));

      ImagePreviews.InitializeCharacter(
          character.Name,
          this.GameContentHelper.Load<Texture2D>,
          contentPackPage.GetModContentHelper().Load<Texture2D>, 
          character.GetModImagePaths,
          character.GetGameImagePaths,
          character.GetPortraitRectsDelegate(),
          character.GetSpriteRectsDelegate());

      this.AddSectionOptions(character)
          .AddComplexOption(
              name: " =  " + I18n.Option_Preview(),
              tooltip: character.GetPreviewTooltip(),
              height: ImagePreviews.GetHeight(character.Name),
              drawAction: (spriteBatch, position) =>
                  ImagePreviews.Draw(character.Name, spriteBatch, position));
    }
  }

  private GenericModConfigMenuIntegration AddSectionOptions(BaseMenuSection section) {
    IEnumerable<BaseMenuSection.OptionItem> sectionOptionItems =
        section.GetOptions().OrderBy(item => item.Property.Name switch {
          "Variant" => 1,
          "Immersion" => 2,
          "WeddingOutfit" => 3,
          _ => 69 // Other options are in the order in which they're defined in their section class.
        });

    foreach (BaseMenuSection.OptionItem item in sectionOptionItems) {
      string displayName = " >  " + item.Name;
      ImagePreviews.SetFieldValue(item.UniqueId, item.Value);

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
