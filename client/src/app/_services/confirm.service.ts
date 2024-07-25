import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { map, Observable } from 'rxjs';

/**
 * Service to handle confirmation dialogs using ngx-bootstrap modals.
 * Provides a method to display a confirmation dialog and return a boolean observable result.
 */
@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  /**
   * Reference to the modal instance.
   */
  bsModalRef?: BsModalRef<ConfirmDialogComponent>;

  /**
   * Constructor to inject the BsModalService for managing modals.
   * @param modalService - Service to handle modal operations.
   */
  constructor(private modalService: BsModalService) { }

  /**
   * Displays a confirmation dialog with customizable options.
   * @param title - The title of the confirmation dialog (default is 'Confirmation').
   * @param message - The message displayed in the confirmation dialog (default is 'Are you sure you want to do this?').
   * @param btnOkText - The text for the confirmation button (default is 'Ok').
   * @param btnCancelText - The text for the cancel button (default is 'Cancel').
   * @returns An observable that emits `true` if the user confirms, `false` otherwise.
   */
  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this?',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    // Configuration object for the modal
    const confif = {
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    };

    // Show the confirmation dialog and get the modal reference
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, confif);

    // Return an observable that emits the result when the modal is hidden
    return this.bsModalRef.onHidden!.pipe(
      map(() => {
        return this.bsModalRef!.content!.result;
      })
    );
  }
}
