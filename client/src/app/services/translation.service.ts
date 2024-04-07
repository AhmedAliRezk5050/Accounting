import {EventEmitter, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {TranslateService} from "@ngx-translate/core";
import AppLanguage from "../shared/models/app-language.enum";

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  languageChanged: EventEmitter<string> = new EventEmitter<string>();

  constructor(private http: HttpClient,
              private translate: TranslateService) {
  }

  initializeTranslation(): void {
    this.translate.addLangs(['en', 'ar']);
    this.translate.setDefaultLang('en');
    const browserLang = this.translate.getBrowserLang();

    // if (browserLang != null) {
    //   this.translate.use(browserLang.match(/en|ar/) ? browserLang : 'en');
    // }
    this.translate.use('ar');
    this.loadTranslationFiles();
  }

  loadTranslationFiles(): void {
    this.http.get(`assets/i18n/${this.translate.currentLang}.json`).subscribe((translations: any) => {
      this.translate.setTranslation(this.translate.currentLang, translations);
    });
  }

  switchLanguage = (language: string) => {
    this.translate.use(language);
    this.languageChanged.emit(language);
  }

  getCurrentLanguage = () => this.translate.currentLang

  isEnglishLanguage = () => this.getCurrentLanguage() === AppLanguage.EN

  getTranslatedWord(key: string): string {
    return this.translate.instant(key);
  }
}
