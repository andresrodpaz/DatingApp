import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Observable<Member[]> | undefined;
  currentIndex = 0;

  constructor(private memberSvc: MembersService) { }

  ngOnInit(): void {
    this.members = this.memberSvc.getMembers();
  }

  // loadMembers() {
  //   this.memberSvc.getMembers().subscribe({
  //     next: members => {
  //       this.members = members;
  //       // Asegurar que el currentIndex no exceda el rango de miembros
  //       this.currentIndex = Math.min(this.currentIndex, this.members.length - 1);
  //     }
  //   });
  // }


}
