import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

/**
 * Directive to conditionally include an Angular template based on user roles.
 * Usage: <div *appHasRole="['Admin', 'Member']">Content for Admins or Members</div>
 */
@Directive({
  selector: '[appHasRole]' // <element appHasRole="['Admin', 'Member']">
})
export class HasRoleDirective implements OnInit {

  /**
   * Input property to specify roles that are allowed to view the template.
   * Usage: *appHasRole="['Admin', 'Member']"
   */
  @Input() appHasRole: string[] = [];

  /**
   * The current user, fetched from the account service.
   */
  user: User | null = null;

  /**
   * Constructor to inject dependencies and initialize the directive.
   * @param viewContainerRef - Reference to the container where the template will be rendered.
   * @param templateRef - Reference to the template that will be conditionally included.
   * @param accountSvc - Service to get the current user.
   */
  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountSvc: AccountService
  ) {
    // Fetch the current user and reevaluate roles when the user is loaded
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
        this.updateView(); // Reevaluate roles when user is loaded
      }
    });
  }

  /**
   * Lifecycle hook that is called after data-bound properties are initialized.
   * Used here to perform the initial evaluation of roles.
   */
  ngOnInit(): void {
    this.updateView(); // Initial evaluation
  }

  /**
   * Method to update the view based on the user's roles.
   * If the user has any of the roles specified in `appHasRole`, the template is rendered.
   * Otherwise, the template is cleared.
   */
  private updateView(): void {
    if (this.user && this.user.roles && this.user.roles.some(role => this.appHasRole.includes(role))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
