import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages?: Message[];
  pagination?: Pagination;
  container = 'Inbox';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  /**
   * Initializes the MessagesComponent with MessageService.
   * @param messageService - Service for managing messages.
   */
  constructor(private messageService: MessageService) { }

  /**
   * Angular lifecycle hook that initializes the component.
   * Loads messages when the component is initialized.
   */
  ngOnInit(): void {
    this.loadMessages();
  }

  /**
   * Loads messages based on the current page number, page size, and container (e.g., Inbox).
   * Sets loading to false once the messages are loaded.
   */
  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe({
      next: response => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      },
      error: err => {
        console.error('Error loading messages:', err);
        this.loading = false;
      }
    });
  }

  /**
   * Deletes a message by its ID.
   * Removes the deleted message from the messages array.
   * @param id - The ID of the message to be deleted.
   */
  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        if (this.messages) {
          const index = this.messages.findIndex(m => m.id === id);
          if (index !== -1) {
            this.messages.splice(index, 1);
          }
        }
      },
      error: err => {
        console.error('Error deleting message:', err);
      }
    });
  }

  /**
   * Handles page change events for pagination.
   * Loads messages for the selected page.
   * @param event - The event object containing the new page number.
   */
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
