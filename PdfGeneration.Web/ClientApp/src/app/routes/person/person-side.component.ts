import {
  Component,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { PersonService } from '../../services';

@Component({
  selector: 'person-side-route',
  templateUrl: 'person-side.component.html',
  providers: [ PersonService ]
})
export class PersonSideComponent implements OnInit {
  personId: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public service: PersonService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(async map => {
      if (map.has('id')) {
        this.personId = parseInt(map.get('id'));

        if (this.personId) {
          const res = await this.service.getPerson(this.personId);
          !res && this.close();
        } else {
          this.close();
        }
      } else {
        this.close();
      }
    });
  }

  close = () => this.router.navigate([{ outlets: { side: null }}]);
}
