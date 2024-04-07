import {Directive, ViewContainerRef} from '@angular/core';

@Directive({
  selector: '[appNode]'
})
export class NodeDirective {

  constructor(public viewContainerRef: ViewContainerRef) { }

}
