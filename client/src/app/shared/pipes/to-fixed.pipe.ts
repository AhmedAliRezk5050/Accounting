import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
  name: 'toFixed'
})
export class ToFixedPipe implements PipeTransform {

  transform(value: number): number {
    return +value.toFixed(2);
  }

}
