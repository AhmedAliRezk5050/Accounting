import {Component, OnInit} from '@angular/core';
import {DataManagementService} from "../../services/data-management.service";
import {AlertService} from "../../../shared/services/alert.service";
import {TranslationService} from "../../../services/translation.service";

@Component({
  selector: 'app-data-management-home',
  templateUrl: './data-management-home.component.html',
  styleUrls: ['./data-management-home.component.scss']
})
export class DataManagementHomeComponent implements OnInit {

  backupFileNames: string[] = [];
  readableDates: string[] = [];

  constructor(private dataManagementService: DataManagementService,
              private alertService: AlertService,
              private translationService: TranslationService
  ) {
  }

  ngOnInit() {
    this.getBackupFileNamesAsDates();
  }

  backupDatabase = () => {
    debugger
    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');

    this.dataManagementService.backupDatabase().subscribe({
      next: _ => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');

        setTimeout(() => {
          this.alertService.alertColor = '';
          this.alertService.alertMsg = '';
          this.alertService.alertShown = false;
        }, 1000);

        this.getBackupFileNamesAsDates();
      },
      error: err => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-add');
      }
    })
  }

  getBackupFileNamesAsDates = () => {
    this.dataManagementService.getBackupFileNames().subscribe({
      next: fileNames => {
        this.backupFileNames = fileNames;

        this.readableDates = fileNames.map(this.formatTimestamp);
      },
      error: err => {
      }
    })
  }

  restoreWithFileName(index: number): void {
    const fileName = this.backupFileNames[index];

    this.alertService.alertShown = true;
    this.alertService.alertColor = 'blue';
    this.alertService.alertMsg = this.translationService.getTranslatedWord('adding');

    this.dataManagementService.restoreDatabase(fileName).subscribe({
      next: (response) => {
        this.alertService.alertColor = 'green';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('success-add');
      },
      error: (err) => {
        this.alertService.alertColor = 'red';
        this.alertService.alertMsg = this.translationService.getTranslatedWord('fail-add');
      }
    });
  }

  formatTimestamp(timestamp: string): string {
    const date = new Date(Number(timestamp) * 1000);

    // You can format the date however you like here
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

}
