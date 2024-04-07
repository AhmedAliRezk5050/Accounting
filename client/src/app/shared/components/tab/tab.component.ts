import {Component, ContentChild, Input, TemplateRef} from '@angular/core';

@Component({
  selector: 'app-tab',
  templateUrl: './tab.component.html',
  styleUrls: ['./tab.component.scss']
})
export class TabComponent {
  @Input() tabTitle = '';
  @Input() active = false;

  @ContentChild('contentTemplateRef', {static: false})
  contentTemplateRef!: TemplateRef<any>;
}
