import {Component, ElementRef, Input, OnDestroy, OnInit} from '@angular/core';
import {ModalService} from "../../services/modal.service";

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit, OnDestroy {
  @Input() modalId!: string
  @Input() maxWidth = 'sm:max-w-lg';

  constructor(public modalService: ModalService, public elementRef: ElementRef) {

  }

  ngOnInit(): void {
    document.body.appendChild(this.elementRef.nativeElement)
  }

  ngOnDestroy(): void {
    document.body.removeChild(this.elementRef.nativeElement)
  }

}
