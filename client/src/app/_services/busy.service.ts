import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount = 0;

  constructor(private spinnerSvc : NgxSpinnerService) { }

  busy(){
    this.busyRequestCount++;
    this.spinnerSvc.show(undefined, {
      type: 'fire',
      bdColor: 'rgba(0, 0, 0, 0.8)',
      color: '#eb6864'
    })
  }
  idle(){
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0){
      this.busyRequestCount = 0;
      this.spinnerSvc.hide();
    }
  }


}
