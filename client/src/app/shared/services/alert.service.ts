import {Injectable, OnInit} from '@angular/core';
import {ModalService} from "./modal.service";

@Injectable({
  providedIn: 'root'
})
export class AlertService{
  alertColor = 'blue';
  alertMsg = '';
  alertShown = false;
  constructor(private modalService: ModalService) { }

}
