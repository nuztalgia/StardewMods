namespace Nuztalgia.StardewMods.DSVCore;

internal sealed class ConfigMenuHelper {

  private static readonly string PreviewLabel = " =  " + I18n.Option_Preview();

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
        .AddParagraph(I18n.Core_Section_Description)
        .AddSectionOptions(coreOptions)
        .AddSpacing();

    foreach (BaseCompatSection section in compatSections) {
      string headerText = section.GetDisplayName();
      string infoText = section.GetInfoText();

      if (section is BaseSyncedCompatSection syncedSection) {
        this.ConfigMenu
            .AddHeaderWithButton(
                headerText: headerText,
                buttonText: I18n.Compat_Synced_OpenMenu(),
                buttonAction: () => this.ConfigMenu.OpenMenuForMod(section.GetModManifest()!))
            .AddParagraph(infoText)
            .AddCharacterThumbnails(syncedSection, this.GameContentHelper.Load<Texture2D>);
      } else {
        this.ConfigMenu
            .AddHeader(headerText)
            .AddParagraph(infoText)
            .AddSectionOptions(section)
            .AddSpacing();
      }
    }

    if (compatSections.IsEmpty() || OnlyCompatSectionIsFlowerQueensCrown(compatSections)) {
      this.ConfigMenu
          .AddHeader(I18n.Compat_Placeholder_Title)
          .AddParagraph(I18n.Compat_Placeholder_Description)
          .AddSpacing();
    }

    static bool OnlyCompatSectionIsFlowerQueensCrown(IEnumerable<BaseCompatSection> sections) {
      return (sections.Count() == 1)
          && (sections.First().GetInfoText() == I18n.Info_FlowerQueensCrown());
    }
  }

  private void SetUpContentPackPage(BaseContentPackPage contentPackPage) {
    this.ConfigMenu.AddPage(contentPackPage.Name, contentPackPage.GetDisplayName());

    foreach (BaseCharacterSection character in contentPackPage.GetAllSections()) {
      CharacterConfigState characterState = CharacterConfigState.Create(
          character.Name,
          this.GameContentHelper.Load<Texture2D>,
          contentPackPage.GetImageLoader(),
          character.GetModImagePaths,
          character.GetGameImagePaths,
          character.GetPortraitRectsDelegate(),
          character.GetSpriteRectsDelegate(),
          character.GetOptions().Select(optionItem => (optionItem.UniqueId, optionItem.Value)));

      this.ConfigMenu
          .AddHeader(character.GetDisplayName().CapitalizeFirstChar())
          .AddSectionOptions(character)
          .AddCharacterPreviews(characterState, PreviewLabel, character.GetPreviewTooltip())
          .AddSpacing();
    }
  }
}
