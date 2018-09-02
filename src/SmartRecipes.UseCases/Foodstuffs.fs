module UseCases.Foodstuffs
    open DataAccess
    open FSharpPlus.Data
    open Infrastructure
    open Models
    open Models.Foodstuff
    open Models.NonEmptyString
    open Infrastructure.Validation
    open Infrastructure.Reader
    open DataAccess.Foodstuffs

    type CreateParameters = {
        name: NonEmptyString
        baseAmount: Amount option
        amountStep: Amount option
    }
    
    type CreateError = 
        | NotAuthorized
        | FoodstuffAlreadyExists
        
    let private createFoodstuff parameters =
        createFoodstuff parameters.name parameters.baseAmount parameters.amountStep
        |> Ok
        |> Reader.id

    let private ensureDoesntAlreadyExists foodstuffDao (foodstuff: Foodstuff) =
        foodstuffDao.search foodstuff.name 
        |> Reader.map (fun fs -> if Seq.isEmpty fs then Ok foodstuff else Error FoodstuffAlreadyExists)
        
    let private addToDatabase foodstuffDao = 
        foodstuffDao.add >> Reader.map Ok

    let create foodstuffDao tokensDao accessToken parameters = 
        Users.authorize tokensDao NotAuthorized accessToken
        >>=! (fun _ -> createFoodstuff parameters)
        >>=! ensureDoesntAlreadyExists foodstuffDao
        >>=! addToDatabase foodstuffDao