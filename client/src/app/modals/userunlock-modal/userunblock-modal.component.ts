import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-userunlock-modal',
  templateUrl: './userunlock-modal.component.html',
  styleUrls: ['./userunlock-modal.component.css']
})
export class UserunblockModalComponent {
  username: string = '';

  constructor(public bsModalRef: BsModalRef) {}

  confirmUnblock() {
    // Aquí iría la lógica para enviar la solicitud de desbloqueo del usuario
    console.log('Unblocking user:', this.username);
    // Puedes agregar el código para enviar la solicitud al servidor aquí
    this.bsModalRef.hide(); // Cerrar el modal después de confirmar
  }
}
