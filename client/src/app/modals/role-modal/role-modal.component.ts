import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-role-modal',
  templateUrl: './role-modal.component.html',
  styleUrls: ['./role-modal.component.css']
})
export class RoleModalComponent implements OnInit {
  username = '';
  availableRoles: string[] = [];
  selectedRoles: string[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}

  updateChecked(checkedValue: string) {
    const index = this.selectedRoles.indexOf(checkedValue);
    if (index === -1) {
      this.selectedRoles.push(checkedValue);
    } else {
      this.selectedRoles.splice(index, 1);
    }
    console.log('Selected roles:', this.selectedRoles); // Para verificar que los roles se actualizan correctamente
  }
}
