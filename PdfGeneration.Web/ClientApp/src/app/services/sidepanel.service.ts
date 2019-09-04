import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class SidepanelService {
  states: string[] = [
    'collapse',
    'thin',
    'full'
  ];

  private state = new BehaviorSubject<string>(this.states[1]);
  state$ = this.state.asObservable();

  toggleState = () => {
    const index = this.states.indexOf(this.state.value);

    index === this.states.length - 1 ?
      this.state.next(this.states[0]) :
      this.state.next(this.states[index + 1]);
  }
}
