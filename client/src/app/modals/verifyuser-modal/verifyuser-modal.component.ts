import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-verifyuser-modal',
  templateUrl: './verifyuser-modal.component.html',
  styleUrls: ['./verifyuser-modal.component.css']
})
export class VerifyuserModalComponent {
  username: string = '';

  constructor(public bsModalRef: BsModalRef) {}

  confirmVerify() {
    // Aquí iría la lógica para enviar la solicitud de verificación del usuario
    console.log('Verifying user:', this.username);
    // Puedes agregar el código para enviar la solicitud al servidor aquí
    this.bsModalRef.hide(); // Cerrar el modal después de confirmar
  }
}
