import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {ModalService} from "../../services/modal.service";
import {AlertService} from "../../services/alert.service";

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent implements OnInit, OnDestroy {
  _bgColor = 'blue'


  constructor(private modalService: ModalService, private alertService: AlertService) {
  }

  @Input() set bgColor(value: string) {
    this._bgColor = `bg-${value}-400`
  }

  ngOnInit(): void {
    this.modalService.modalVisibilityChanged.subscribe(status => {
      if (!status) {
        this.alertService.alertMsg = ''
        this.alertService.alertShown = false;
      }
    })
  }

  ngOnDestroy(): void {
    this.alertService.alertMsg = '';
    this.alertService.alertColor = 'blue';
  }
}
