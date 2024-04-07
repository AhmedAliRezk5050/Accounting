import {Component} from '@angular/core';
import {TranslationService} from "../../services/translation.service";
import appLanguageEnum from "../../shared/models/app-language.enum";

@Component({
  selector: 'app-language-switcher',
  templateUrl: './language-switcher.component.html',
  styleUrls: ['./language-switcher.component.scss']
})
export class LanguageSwitcherComponent {
  constructor(private translationService: TranslationService) {
  }

  switchLanguage(event: Event): void {
    const lang = (event.target as HTMLSelectElement).value;
    this.translationService.switchLanguage(lang);
  }

  get isArabic() {
    return this.translationService.getCurrentLanguage() === appLanguageEnum.AR
  }

  get isEnglish() {
    return this.translationService.isEnglishLanguage()
  }
}
