import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {CustomDateParser} from '../pipes/custom-date-parser.pipe';


import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs'
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { NgxSpinner, NgxSpinnerModule } from 'ngx-spinner';

@NgModule({
  declarations: [CustomDateParser],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-center'
    }),
    CarouselModule.forRoot(),
    NgxSpinnerModule.forRoot({
      type: 'fire'
    })
  ],
  exports:[
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    CustomDateParser,
    NgxSpinnerModule
  ]
})
export class SharedModule { }
