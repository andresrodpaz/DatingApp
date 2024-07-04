import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]' //<appHasRole = Admin, Member ...
})
export class HasRoleDirective implements OnInit{

  @Input() appHasRole: string[] = [];
  user: User | null = null;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountSvc: AccountService) {
    this.accountSvc.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        this.user = user;
        this.updateView(); // Reevaluate roles when user is loaded
      }
    });
  }

  ngOnInit(): void {
    this.updateView(); // Initial evaluation
  }

  private updateView(): void {
    if (this.user && this.user.roles && this.user.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
