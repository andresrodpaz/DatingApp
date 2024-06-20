import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {CustomDateParser} from '../pipes/custom-date-parser.pipe';


import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs'
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { NgxSpinner, NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import {PaginationModule} from 'ngx-bootstrap/pagination'
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from "ngx-timeago";

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
    }),
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot()
  ],
  exports:[
    BsDropdownModule,
    ToastrModule,
    TabsModule,
    CustomDateParser,
    NgxSpinnerModule,
    FileUploadModule,
    BsDatepickerModule,
    PaginationModule,
    ButtonsModule,
    TimeagoModule
  ]
})
export class SharedModule { }
