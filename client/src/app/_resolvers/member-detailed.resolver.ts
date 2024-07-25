import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

/**
 * Resolver to fetch detailed information about a member before activating a route.
 */
export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MembersService);

  // Retrieve the 'username' parameter from the route and fetch the member details
  return memberService.getMember(route.paramMap.get('username')!);
};
