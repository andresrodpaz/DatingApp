import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private accountService: AccountService) {

  }
  ngOnInit(): void {
    //this.getUsers();
    this.setCurrentUser();
  }

  // getUsers(){
  //   this.http.get('http://localhost:5000/api/users').subscribe(
  //     {
  //       next: response => this.users = response,
  //       error: error => console.log(error),
  //       complete: () => console.log('Request has completed!!')
  //     }
  //   );
  // }

  setCurrentUser(){
    //const user:User = JSON.parse(localStorage.getItem('user')!);

    const userString = localStorage.getItem('user');
    if (!userString){
      return;
    }
    const user : User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }

  title = 'HumanConnect';
}
