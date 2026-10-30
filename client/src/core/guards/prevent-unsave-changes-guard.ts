import { CanDeactivateFn } from '@angular/router';
import { MemberProfile } from '../../features/members/member-profile/member-profile';

// <MemberProfile> represents the type of component we are creating this guard for
export const preventUnsaveChangesGuard: CanDeactivateFn<MemberProfile> = (
  component // component we are trying to move away from (in this case will be MemberProfile)
) => {
  // if the form was edited 
  if (component.editForm?.dirty) {
    return confirm("Are you sure you want to continue? All unsaved changes will be lost")
  }
  // proceed with navigation
  return true;
};
