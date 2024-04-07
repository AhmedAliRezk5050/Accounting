import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'negativeToParentheses'
})
export class NegativeToParenthesesPipe implements PipeTransform {

  transform(value: number | string): string {

    let num: number = 0;
    if(typeof value == 'string') {
      num  = parseFloat(value.replace(/,/g, ''));
    } else {
      num = value;
    }

    return num < 0 ? `(${Math.abs(num).toLocaleString('en-US')})` : `${value}`;
  }

}
