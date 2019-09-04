import {
  Component,
  OnInit,
  OnDestroy
} from '@angular/core';

import { SidepanelService } from '../../services';
import { Subscription } from 'rxjs';

import {
  trigger,
  state,
  transition,
  style,
  animate
} from '@angular/animations';

@Component({
  selector: 'sidepanel',
  templateUrl: 'sidepanel.component.html',
  styleUrls: ['sidepanel.component.css'],
  animations: [
    trigger(
      'slide', [
        state('collapse', style({
          width: 0,
          opacity: 0
        })),
        state('thin', style({
          width: '100%',
          opacity: 1
        })),
        state('full', style({
          width: '100%',
          opacity: 1
        })),
        transition('collapse => thin', animate('100ms ease-in')),
        transition('thin => full', animate('100ms ease-in')),
        transition('full => collapse', animate('100ms ease-out'))
      ]
    )
  ]
})
export class SidepanelComponent implements OnInit, OnDestroy {
  private sub: Subscription;
  state: string;
  constructor(
    public sidepanel: SidepanelService
  ) { }

  ngOnInit() {
    this.sub = this.sidepanel.state$.subscribe((s: string) => this.state = s);
  }

  ngOnDestroy() {
    // tslint:disable-next-line: no-unused-expression
    this.sub && this.sub.unsubscribe();
  }
}
