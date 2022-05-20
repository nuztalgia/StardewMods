namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class ConfigMenuHelper {

  private static readonly string PreviewLabel = " =  " + I18n.Option_Preview();
  private static readonly Func<string, string> FormatValue =
      value => I18nHelper.GetStringByKeyName($"Value_{value}") ?? value;

  private readonly GenericModConfigMenuIntegration ConfigMenu;
  private readonly IGameContentHelper GameContentHelper;

  internal ConfigMenuHelper(
      GenericModConfigMenuIntegration configMenu, IGameContentHelper gameContentHelper) {
    this.ConfigMenu = configMenu;
    this.GameContentHelper = gameContentHelper;
  }

  internal void SetUpMenuPages(
      CoreAndCompatPage coreAndCompatPage, 
      IEnumerable<BaseContentPackPage> installedContentPackPages,
      IEnumerable<BaseContentPackPage> otherContentPackPages) {

    this.ConfigMenu.OptionNamePrefix = ">";
    this.ConfigMenu.PageLinkPrefix = ">";

    this.ConfigMenu.OnFieldChanged = (string fieldId, object newValue) => {
      Log.Trace($"Field '{fieldId}' was changed to: '{newValue}'.");
      CharacterConfigState.Update(fieldId, newValue);
    };

    this.SetUpMainPage(coreAndCompatPage, installedContentPackPages, otherContentPackPages);
    this.SetUpCoreAndCompatPage(coreAndCompatPage);
    installedContentPackPages.ForEach(page => this.SetUpContentPackPage(page));
  }

  private void SetUpMainPage(
      CoreAndCompatPage coreAndCompatPage,
      IEnumerable<BaseContentPackPage> installedContentPackPages,
      IEnumerable<BaseContentPackPage> otherContentPackPages) {

    this.ConfigMenu
        .AddHeader(I18n.Main_Intro_Title)
        .AddParagraph(I18n.Main_Intro_Description)
        .AddPageLink(coreAndCompatPage.Name, coreAndCompatPage.GetDisplayName())
        .AddSpacing()
        .AddHeader(I18n.Main_InstalledPacks_Title)
        .AddParagraph(installedContentPackPages.Any()
            ? I18n.Main_InstalledPacks_Description
            : I18n.Main_InstalledPacks_None);

    foreach (BaseContentPackPage contentPackPage in installedContentPackPages) {
      Log.Trace($"'{contentPackPage.GetDisplayName()}' pack is installed. Adding config menu.");
      this.ConfigMenu.AddPageLink(contentPackPage.Name, contentPackPage.GetDisplayName());
    }

    this.ConfigMenu
        .AddSpacing()
        .AddHeader(I18n.Main_OtherPacks_Title)
        .AddParagraph(otherContentPackPages.Any()
            ? I18n.Main_OtherPacks_Description
            : I18n.Main_OtherPacks_None);

    foreach (BaseContentPackPage contentPackPage in otherContentPackPages) {
      Log.Trace($"'{contentPackPage.GetDisplayName()}' pack is *NOT* installed.");
      this.ConfigMenu.AddHeaderWithPrefix(contentPackPage.GetDisplayName(), "*");
    }
  }

  private void SetUpCoreAndCompatPage(CoreAndCompatPage coreAndCompatPage) {
    BaseMenuSection coreOptions = coreAndCompatPage.CoreOptions;
    IEnumerable<BaseCompatSection> compatSections = coreAndCompatPage.GetAvailableCompatSections();

    this.ConfigMenu
        .AddPage(coreAndCompatPage.Name, coreAndCompatPage.GetDisplayName())
        .AddHeader(coreOptions.GetDisplayName())
        .AddParagraph(I18n.Core_Section_Description);

    this.AddSectionOptions(coreOptions)
        .AddSpacing();

    foreach (BaseCompatSection section in compatSections) {
      this.ConfigMenu
          .AddCompatSectionHeader(section)
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
          .AddHeader(I18n.Compat_Placeholder_Title)
          .AddParagraph(I18n.Compat_Placeholder_Description);
    }
  }

  private void SetUpContentPackPage(BaseContentPackPage contentPackPage) {
    this.ConfigMenu.AddPage(contentPackPage.Name, contentPackPage.GetDisplayName());

    foreach (BaseCharacterSection character in contentPackPage.GetAllSections()) {
      this.ConfigMenu.AddHeader(character.GetDisplayName().CapitalizeFirstChar());

      CharacterConfigState characterState = CharacterConfigState.Create(
          character.Name,
          this.GameContentHelper.Load<Texture2D>,
          contentPackPage.GetImageLoader(),
          character.GetModImagePaths,
          character.GetGameImagePaths,
          character.GetPortraitRectsDelegate(),
          character.GetSpriteRectsDelegate(),
          character.GetOptions().Select(optionItem => (optionItem.UniqueId, optionItem.Value)));

      this.AddSectionOptions(character)
          .AddCharacterPreviews(characterState, PreviewLabel, character.GetPreviewTooltip())
          .AddSpacing();
    }
  }

  private GenericModConfigMenuIntegration AddSectionOptions(BaseMenuSection section) {
    foreach (BaseMenuSection.OptionItem item in section.GetOptions()) {
      switch (item.Value) {
        case Enum: {
          this.ConfigMenu.AddEnumOption(
              section, item.Property, item.Name, FormatValue, item.Tooltip, item.UniqueId);
          break;
        }
        case bool: {
          this.ConfigMenu.AddCheckbox(
              section, item.Property, item.Name, item.Tooltip, item.UniqueId);
          break;
        }
        case int value: {
          this.ConfigMenu.AddSlider(
              section, item.Property, item.Name, item.Tooltip, item.UniqueId,
              staticMinValue: section.GetMinValue(item.Property),
              getDynamicMaxValue: () => section.GetMaxValue(item.Property));
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
