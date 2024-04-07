import {ChangeDetectorRef, Component, OnInit, Renderer2} from '@angular/core';
import {ModalService} from "./shared/services/modal.service";
import {TranslationService} from './services/translation.service';
import AppLanguage from "./shared/models/app-language.enum";
import {environment} from "../environments/environment";
import {LoadingSpinnerService} from "./shared/services/loading-spinner.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  isLoading = false;

  constructor(
    private cdRef: ChangeDetectorRef,
    public modalService: ModalService,
    private translationService: TranslationService,
    private renderer: Renderer2,
    public loadingSpinnerService : LoadingSpinnerService
  ) {
    console.log(environment.apiUrl)
    console.log(environment.production)
  }

  ngOnInit(): void {
    this.translationService.initializeTranslation();

    this.changeAppDir(this.translationService.getCurrentLanguage())

    this.translationService.languageChanged.subscribe(lang => {
      this.changeAppDir(lang)
    })

    this.loadingSpinnerService.loading$.subscribe({
      next: val => {
        this.isLoading = val;
        this.cdRef.detectChanges();
      }
    })
  }

  changeAppDir = (lang:string) => {
    if (lang === AppLanguage.EN) {
      this.renderer.setAttribute(document.documentElement, 'dir', 'ltr');
      this.renderer.setAttribute(document.documentElement, 'lang', 'en');
    } else if (lang === AppLanguage.AR){
      this.renderer.setAttribute(document.documentElement, 'dir', 'rtl');
      this.renderer.setAttribute(document.documentElement, 'lang', 'ar');
    }
  }

  // get isLoading() {
  //   return this.loadingSpinnerService.loading$
  // }
}
