import {Component, EventEmitter, Input, Output} from '@angular/core';
import {DropdownService} from "../../../services/dropdown.service";

@Component({
  selector: 'app-drop-down',
  templateUrl: './drop-down.component.html',
  styleUrls: ['./drop-down.component.scss']
})
export class DropDownComponent {
  @Input() apiEndpoint = '';
  @Input() dataFormat: 'array' | 'object' = 'array'; // Default to 'array'
  @Output() selectedItem: EventEmitter<any> = new EventEmitter<any>();

  dropdownItems: any[] = [];
  filteredItems: any[] = [];
  searchQuery = '';

  constructor(private dropdownService: DropdownService) { }

  ngOnInit(): void {
    this.loadDropdownItems();
  }

  loadDropdownItems() {
    this.dropdownService.getDropdownItems(this.apiEndpoint, this.dataFormat).subscribe((data: any) => {
      // Check the data format and assign the appropriate property to 'dropdownItems'
      this.dropdownItems = this.dataFormat === 'array' ? data : Object.values(data);
      this.filteredItems = this.dropdownItems;
    });
  }

  onSearch() {
    this.filteredItems = this.dropdownItems.filter(item =>
      item.name.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  onItemClick(item: any) {
    this.selectedItem.emit(item);
  }
}
