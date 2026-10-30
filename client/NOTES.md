# Signals and Observables
## Signals 
There are a number of signals in angular - the one you are likely most familar with and the one most used is Writables signals 

### Writable Signals 
Signals that provide an API for updating their value directly 
``` javascript
const count = signal(0)
...
console.log(count()) // get value of signal
...
count.set(300) // set value of signal
```

To update a signal value based on the previous value, use the `update` method `count.update(prevVal => prevVal + 2)`

### Computed Signsls 
Signals that derive their value based off another signal and are readonly 
``` javascript
const doubleCount = computed(() => count() * 2)
```
## Observables
A way to manage async data, giving us a way to listen to multiple values over time 

Angular's httpClient returns observables 

Observables come from the RxJS library 

Map functions can be used to tranforms data from an observable
``` javascript
getMmebers() {
    return this.http.get('api/members').pipe(
        map(members => {
            return member.id
        })
    )
}
```

Observables are lazy - they will only submit a change if there are subscribed to
``` javascript
getMembers().subscribe({
    members => {}, // what to do with the next piece of data
    error => {}, // what to do if there is an error
    () => {} // what to do when complete (once all data for observable has concluded)
})
```

Http requests ALWAYS complete - there is only a single data object. Because of this we can instead make use of Promises 
``` javascript
getMemebers() {
    return this.http.get('api/users').toPromise()
}
```

This way we dont need to worry about unsubscribing 

Although the above is strictly speaking more fitting for http requests, its not really necessary - http requests ALWAYS complete so we dont need to unsubscribe ourselves 

The async pipe can be used to subscribe and unsubscribe to and from observables inside of Angular templates 
``` html
@for (member of members$ | async; track member.id) {
    <div>{{member.displayName}}</div>
}
```

Like any pipe, `async` is not available to a template by default. A standalone component (the default since Angular 19) can only use the pipes, directives and components listed in its own `imports` array, so `AsyncPipe` must be imported from `@angular/common` and added there - otherwise the template fails to compile with `NG8004: No pipe found with name 'async'`
``` javascript
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-member-list',
  imports: [AsyncPipe],
  templateUrl: './member-list.html',
})
export class MemberList {
  protected members$ = inject(MemberService).getMembers(); // the $ suffix is the naming convention for observables
}
```

see [member-list.html](./src/features/members/member-list/member-list.html#2) for the async pipe in use and [member-list.ts](./src/features/members/member-list/member-list.ts#9) for the `AsyncPipe` import

## Signals 
`Signals` is an alternative to RXJs's Observables introduced in Angular 16 (although it does not replace async functionality of observables)

A wrapper around a value that notifies interested consumers 
``` javascript
const count = signal(0) // initial value of 0 

// signals are getter functions to get the value of signal - execute it 
console.log(count())

// set the value of signal 
count.set(3)

// update value 
count.update(value => calue + 120) // value is the value of the signal before the change, the return is the new value 
```

Signals can be used for state management and allows us to use them in place of observables (although you must still keep in mind signals != observables)

Benefits over using Obserables:
* Performant 
* Simple and readable
* No need to unsubscribe 
* Preditable - they are updated synchronously (thats why they are not the same as Observables)

# Input properties 
A way to provide input from parent components to children components. There are 2 ways to do this:
1. Input signals - see [register.ts](./src/features/account/register/register.ts#14)
2. Using `@Input` decorator (probably wont use this as the future of Angular is signals) - see [home.tx](./src/features/home/home.ts#12)

# Output properties
A way for a child to expose data to parents for them to react on. Just like with input properties there are 2 ways to provide output properties:
1. Output signal - see [register.ts](./src/features/account/register/register.ts#15) to see how it is defined by child and see [home.html](./src/features/home/home.html#21) to see how it is used by parent
2. Using the `@Output` decorator (probably wont use this as the future of Angular is signals)

# Routing 
For single page applications, routing is a way to allow moving between components of our application. Angular provides routing out of the box and we can define routes for our application in the [app.routes.ts](./src/app/app.routes.ts) file, specifying the path and the component to be displayed when that route is traversed

## Navigating to Routes from HTML
We can navigate to a route in code by injecting the `Router` dependency into our component that wishes to perform routing as has been done on [line 16](./src/layout/nav/nav.ts#16) of nav.ts. 

The `<a>` html elements defined in [nav.html](./src/layout/nav/nav.html#21) all define the `routerLink` property that when clicked will route to the provided URL. Furthermore, for styling the `routerLinkActive` property supplies the class to apply when the route is active

A caveat with `routerLinkActive` is that exact path match is not true by default thus [nav.html line 8](./src/layout/nav/nav.html#8) defines the `routerLinkActiveOptions` input property 
to provide exact match true as option to apply class ONLY if the path matches exactly

## Navigating to Routes from Code
We can navigate to a route in code by injecting the `Router` dependency into our component that wishes to perform routing as has been done on [line 16](./src/layout/nav/nav.ts#16) of nav.ts. 

We can then use this dependency to navigate to any of the paths defined in `app.routes.ts` - [line 22](./src/layout/nav/nav.ts#22)

## Route Guards
Route guards are functions that control whether a user can navigate to or leave a particular route.

To create a guard we can use `npx ng g g` - you can then select the type of guard an activation or deactivation guard (there are many more)

### Activation Guard
Really just user interface hiding based on a condition - true security comes from backend where authentication takes place

We defined a `canActivate` guard which created the guard [auth-guard.ts](./src/core/guards/auth-guard.ts). So long as this guard function returns true, the route that uses it will be allowed to be navigated 

We used this guard in the routes by providing the guard in the array of `canActivate` property of the paths in [app.routes.ts](./src/app/app.routes.ts)

TO define a guard for multiple routes we can specify a parent route and provide it with children and apply the guard to the parent. 

### Deactivation Guards
See [prevent-unsave-changes-guard.ts](./src/core/guards/prevent-unsave-changes-guard.ts) for a defintion of a deactivation guard and see how it is added to the `MemberProfile` component route in [prevent-unsave-changes-guard.ts](./src/app/app.routes.ts).

Something to note - deactivation guards work within our application - i.e. we can define a guard to allow a user to move away from a component without our app - but they dont outside of our app (i.e. when the user navigates away from our component within the browser itself, like reloading the page)

There is some trickery to add this functionality too - see [MemberFofile](./src/features/members/member-profile/member-profile.ts#24) for this

## Navigation Extras
When navigating a route, we can provide the navigation method with `NavigationExtras` - this is only ever non-null when the path is navigated, all other times it is null - see [server-error.ts](./src/shared/errors/server-error/server-error.ts). For this reason we can only access it in constructors of components.

To provide a path with NavigationExtras (state is a navigation extra to provide), provide the `NavigationExtras` object to the path being navigated:
``` javascript
const navigationExtras: NavigationExtras = { state: { error: error.error } }
router.navigateByUrl('/server-error', navigationExtras)         
```

see [error-interceptor.ts](./src/core/interceptors/error-interceptor.ts#36) for more

## Route Parameters
A route parameter is a dynamic segment of a URL (e.g. the `5` in `/members/5`) that a component reads through the injected `ActivatedRoute` service.

A parameter is declared in the route's path with a `:` prefix - `{ path: 'members/:id', component: MemberDetailed }` matches `/members/5`, `/members/abc` etc. and stores whatever is in that segment under the name `id`. `ActivatedRoute` describes the route that loaded the current component (its params, query params, data, parent route etc.), so a component injects it to find out which `id` it was opened with - typically the id of the record to fetch from the API.

There are 2 ways to read a parameter:
1. `route.snapshot.paramMap.get('id')` - reads the value once, as it was when the component was created
2. `route.paramMap` - an observable that emits a new value every time the parameter changes

Values are always `string | null` (`null` if no parameter has that name), so convert them yourself if a number is needed. The difference between the two matters when navigating from `/members/1` to `/members/2`: the route stays the same, so Angular reuses the existing component instead of creating a new one - the constructor doesn't run again and the snapshot still holds `1`, while the `paramMap` observable emits `2`.

Query string values (`/members?page=2`) are read the same way from `route.queryParamMap`. Alternatively, adding `withComponentInputBinding()` to `provideRouter` in `app.config.ts` makes Angular bind route parameters straight to component inputs of the same name (e.g. `id = input<string>()`), without injecting `ActivatedRoute`.
``` javascript
export class MemberDetailed {
  private route = inject(ActivatedRoute);

  constructor() {
    const id = this.route.snapshot.paramMap.get('id'); // read once
    this.route.paramMap.subscribe(params => console.log(params.get('id'))); // read on every change
  }
}
```

see [member-detailed.ts](./src/features/members/member-detailed/member-detailed.ts#13) for `ActivatedRoute` being injected and [app.routes.ts](./src/app/app.routes.ts#22) for the `members/:id` route it reads from

## Router Resolver 
We can make use of a router resolver to load data before a route is navigated to (before it is activated).

To create a new resolver we can make use of the `ng g resolver <resolver-name>` command

We define our resolver as has been done in [member-resolver.ts](./src/features/members/member-resolver.ts).

We can then use the resolver in our [app.routes.ts](./src//app/app.routes.ts#28)

To access data the resolver has provided for us we can simply subscribe to the data property of route 
``` typescript
private route = inject(ActivatedRoute)
this.route.data.subscribe({
    next: (data) => console.log(data['member']) // member is they key provided for the data returned by resolver
})
```

see [member-detailed.ts](./src/features/members/member-detailed/member-detailed.ts) for more

# Pipes 
Pipes are special operators in Angular template expressions that allow you to transform data declaratively in your template

There are the Angular provided pipes:
1. AsyncPipe - see [member-profile.html](./src/features/members/member-profile/member-profile.html) for how it is used. Used to register and unregister to an observable
2. DatePipe - see [member-profile.html](./src/features/members/member-profile/member-profile.html) for how it is used. Used to format date strings

## Custom Pipes
We cna define our own pipes using the command:
``` bash
ng g p <pipe-name>
```


# JavaScript
## Getters
A `get` accessor is a JavaScript feature (not TypeScript or Angular) - a method that is read like a property (`photoMocks`, not `photoMocks()`) and whose body runs again on **every** read, with nothing cached.
* **TypeScript** only adds types on top - a return type, access modifiers (`protected get ...`), and a getter without a setter is read-only
* **Angular** doesn't change how getters work but reads template bindings on every change detection cycle, so a getter used in a template re-runs often - use a plain field or a `computed()` signal for anything expensive

see [member-photos.ts](./src/features/members/member-photos/member-photos.ts#28) for a getter used by the `@for` in [member-photos.html](./src/features/members/member-photos/member-photos.html#8)

# FAQ
## Why does `routerLinkActiveOptions` need `[]` but `routerLink` and `routerLinkActive` don't?
The square brackets decide how Angular reads the value:
* `attr="value"` - the value is passed as a **plain string**
* `[attr]="expr"` - the value is **evaluated as an expression** (object, array, number, boolean, component variable, etc.)

`routerLink` and `routerLinkActive` both accept strings (a URL and a class name), so no brackets are needed. `routerLinkActiveOptions` expects an **object**, so without brackets it would receive the string `"{exact: true}"` instead of the object `{exact: true}` - see [nav.html line 8](./src/layout/nav/nav.html#8)

``` html
<a routerLink="/" routerLinkActive="text-accent" [routerLinkActiveOptions]="{exact: true}">
```

Brackets can still be used on string inputs when the value is dynamic, e.g. `[routerLink]="['/members', member.id]"`

## Can you `throw` something that isn't an `Error` (e.g. an array of strings)?
In JavaScript/TypeScript, `throw` isn't limited to `Error` objects - **any value** can be thrown (string, number, object, array). Whatever is thrown is what the `catch` receives, which is why caught errors are typed `unknown`/`any`
``` javascript
throw "oops";            // valid
throw 42;                // valid
throw ["a", "b"];        // valid
throw new Error("x");    // the usual case, but not required
```

## Why doesn't a validator that compares two form controls update when the other control changes?
A control's validators only re-run when **that control's own value** changes. If the validator reads another control's value, changing that other control does not re-validate it, so it keeps a stale error. Put the comparison validator on one control only, and re-run its validation whenever the control it compares to changes
``` typescript
this.form = new FormGroup({
  password: new FormControl('', [Validators.required]),
  confirmPassword: new FormControl('', [Validators.required, matchValues('password')])
});

this.form.controls['password'].valueChanges.subscribe(() => {
  this.form.controls['confirmPassword'].updateValueAndValidity();
});
```

# HTTP Interceptors 
Interceptors can be created using  `ng generate interceptor` command. An interceptor are used to intercept and modify HTTP requests and response before they reach their desitnation, allowing authentication handling, exception handling, logging etc. (see `ng g --help` interceptor component for more)

This is very similar to @ControllerAdvice in spring boot and Middleware in ASP.NET

To use the interceptor we have to register it with the `provideHttpClient` provider in [app.config.ts](./src//app/app.config.ts#15)

# Environments 
Angular does not generate environment files anymore :O. We can however generate environment files using the `ng g environments` command. This will update [angular.json](./angular.json) to include:
``` json
...
"development": {
    "optimization": false,
    "extractLicenses": false,
    "sourceMap": true,
    "fileReplacements": [
    {
        "replace": "src/environments/environment.ts",
        "with": "src/environments/environment.development.ts"
    }
    ]
}
...
```

as well as generate the files [environment.ts](./src/environments/environment.ts) for prod environment variables to be defined and [environment.development.ts](./src/environments/environment.development.ts) for dev environment variables

These environment files are simple variables that can be imported and used by the application to reference environment variables where applicable

_*Note: always import environment.ts file and NOT environment.development.ts file as environment.development.ts will replace environment.ts in development mode (vice versa is NOT true) according to [angular.json](./angular.json#67)*_

# Forms 
## Template-Driven Forms 

## Reactive Forms 
Advanced form usage that is component driven given full control over validation form state and data in the form

### Custome Validators
See [register.ts matchValues function](./src/features/account/register/register.ts#54) for the defining of a custom validator. 

To use this validator, we provide it in the list of validators for a form control - see [confirmPassword form control](./src/features/account/register/register.ts#41) for more

### Defining a Reusable Form Control 
See [text-input](./src/shared/text-input)

### Form Builders 
Creating new instances of form controls for a `FormGroup` can become tedious - enter `FormBuilder`

It makes a difference but not much in my opinion 