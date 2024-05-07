import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();

  model:any = {};

  constructor(private accountSvc:AccountService, private toast:ToastrService) { }

  ngOnInit(): void {

  }

  register(){
    console.log("Register: " , this.model);
    this.accountSvc.register(this.model).subscribe({
      next: () => {
        //console.log(response);
        this.cancel();
      },
      error: error => {
        console.log(error);
        this.toast.error(error.error);
      }
    })
  }

  cancel(){
    console.log("Cancelled");
    this.cancelRegister.emit(false);
  }
}
