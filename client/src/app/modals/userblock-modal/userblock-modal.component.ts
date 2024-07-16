import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-userblock-modal',
  templateUrl: './userblock-modal.component.html',
  styleUrls: ['./userblock-modal.component.css']
})
export class UserblockModalComponent {
  username: string = '';

  constructor(public bsModalRef: BsModalRef) {}

  confirmBlock() {
    // Aquí iría la lógica para enviar la solicitud de bloqueo del usuario
    console.log('Blocking user:', this.username);
    // Puedes agregar el código para enviar la solicitud al servidor aquí
    this.bsModalRef.hide(); // Cerrar el modal después de confirmar
  }
}
