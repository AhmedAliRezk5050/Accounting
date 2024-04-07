import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  QueryList,
  ViewChild,
  ViewChildren
} from '@angular/core';
import {debounceTime, fromEvent, map} from "rxjs";
import {AlertComponent} from "../alert/alert.component";

@Component({
  selector: 'app-data-list-search-input',
  templateUrl: './data-list-search-input.component.html',
  styleUrls: ['./data-list-search-input.component.scss']
})
export class DataListSearchInputComponent implements OnInit {


  @Input() placeholder = 'Search';
  @Input() items: { text: string, value: string }[] = []
  @Output() searchStarted = new EventEmitter<string>();
  @Output() valueSelected = new EventEmitter<string>();
  @ViewChild('searchInput', {static: true}) searchInput?: ElementRef;
  listShown = false
  @Input() selectedItem: { text: string, value: string } = {text: '', value: ''}
  textInputShown = true;

  ngOnInit(): void {
    fromEvent(this.searchInput!.nativeElement, 'keyup')
      .pipe(
        debounceTime(500),
        map((event: any) => event.target.value)
      ).subscribe(val => {
      if (val.trim()) {
        this.searchStarted.emit((val));
      } else {
        this.items = [];
      }
      this.listShown = true;
    })
  }

  onValueSelected(item: { text: string, value: string }) {
    this.selectedItem = item
    this.valueSelected.emit(item.value)
    this.listShown = false
    this.updateTextInputShownStatus(false)
  }

  updateTextInputShownStatus = (status: boolean) => {
    this.textInputShown = status
  }
}
