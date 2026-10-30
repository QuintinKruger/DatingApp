import { Routes } from '@angular/router';
import { Home } from '../features/home/home';
import { MemberList } from '../features/members/member-list/member-list';
import { MemberDetailed } from '../features/members/member-detailed/member-detailed';
import { Lists } from '../features/lists/lists';
import { Messages } from '../features/messages/messages';
import { authGuard } from '../core/guards/auth-guard';
import { TestErrors } from '../features/test-errors/test-errors';
import { NotFound } from '../shared/errors/not-found/not-found';
import { ServerError } from '../shared/errors/server-error/server-error';
import { MemberProfile } from '../features/members/member-profile/member-profile';
import { MemberPhotos } from '../features/members/member-photos/member-photos';
import { MemberMessages } from '../features/members/member-messages/member-messages';
import { memberResolver } from '../features/members/member-resolver';
import { preventUnsaveChangesGuard } from '../core/guards/prevent-unsave-changes-guard';

// Angular's router checks routes in array order and uses the first one that matches. It doesn't look for the best match.
// thus anything below { path: '**', component: Home } will not be routed to as the wild card wins out always 
export const routes: Routes = [
    { path: '', component: Home },
    {
        path: '',
        runGuardsAndResolvers: 'always', // always run the authGaurd 
        canActivate: [authGuard], // all guards need to return true to allow navigation to this route
        children: [
            { path: 'members', component: MemberList },
            { 
                path: 'members/:id', 
                resolve: {member:memberResolver}, // key of what we are resolving, this will be available to all children components too
                runGuardsAndResolvers: 'always', 
                component: MemberDetailed ,
                children: [
                    {path: '', redirectTo: 'profile', pathMatch: 'full'},
                    {path: 'profile', component: MemberProfile, title: 'Profile', canDeactivate: [preventUnsaveChangesGuard]}, // deactivate guard to prevent moving away from route based on definition of provided guard
                    {path: 'photos', component: MemberPhotos, title: 'Photos'},
                    {path: 'messages', component: MemberMessages, title: 'Messages'}
                ]
            }, // :id is a dynamic route parameter 
            { path: 'lists', component: Lists },
            { path: 'messages', component: Messages },
        ]
    },
    // { path: 'members', component: MemberList, canActivate: [authGuard] }, // defining a guard for a single route            
    { path: 'errors', component: TestErrors },
    { path: 'server-error', component: ServerError },
    { path: '**', component: NotFound } // wild card route - going to a route that does exist
];
