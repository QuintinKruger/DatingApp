import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/member-service';
import { Member } from '../../types/member';
import { EMPTY } from 'rxjs';

export const memberResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MemberService);
  const router = inject(Router);

  const memberId = route.paramMap.get("id");
  if (!memberId) { 
    router.navigateByUrl("/not-found") 
    return EMPTY // weird and this is just satisfying typescript, we typically dont expect this code to run anyway - if was introduced again to satisfy typescript
  };
  return memberService.getMember(memberId);
};
